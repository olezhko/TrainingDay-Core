import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { ExerciseTags, ExerciseTagsEnglishLabels } from 'src/app/data/exercises/exercise-tags';
import { MUSCLE_GROUPS, MusclesEnumEnglishLabels, MusclesEnum } from 'src/app/data/exercises/exercise-muscle';
import { AuthService } from 'src/app/services/auth/auth.service';
import { ExercisePreviewComponent } from '../exercise-preview/exercise-preview.component';
import * as ExerciseActions from 'src/app/services/ngrx/actions/exercise.actions';
import { selectExerciseList, selectIsLoading } from 'src/app/services/ngrx/actions/exercise.selectors';

export interface MuscleOption {
  value: number;
  label: string;
  selected: boolean;
}

export interface MuscleGroupSection {
  label: string;
  options: MuscleOption[];
}

interface TagOption {
  value: number;
  label: string;
  selected: boolean;
}

interface DifficultyOption {
  value: number;
  label: string;
  selected: boolean;
}

@Component({
  selector: 'app-exercise-list',
  standalone: true,
  imports: [CommonModule, MatIconModule, ExercisePreviewComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './exercise-list.component.html',
  styleUrls: ['./exercise-list.component.css']
})
export class ExerciseListComponent implements OnInit {
  private store = inject(Store);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  public authService = inject(AuthService);

  public filterName: string = '';
  public exercisePreviews = toSignal(this.store.select(selectExerciseList), { initialValue: [] as ExercisePreview[] });
  public loading = toSignal(this.store.select(selectIsLoading), { initialValue: false });
  public filtersExpanded = signal(false);

  public muscleGroups: MuscleGroupSection[] = MUSCLE_GROUPS.map(g => ({
    label: g.label,
    options: g.muscles.map(m => ({
      value: Number(m),
      label: MusclesEnumEnglishLabels[m as MusclesEnum],
      selected: false
    }))
  }));

  public tagOptions: TagOption[] = Object.values(ExerciseTags).map(v => ({
    value: Number(v),
    label: ExerciseTagsEnglishLabels[v as ExerciseTags],
    selected: false
  }));

  public difficultyOptions: DifficultyOption[] = [
    { value: 1, label: 'Easy', selected: false },
    { value: 2, label: 'Medium', selected: false },
    { value: 3, label: 'Hard', selected: false },
  ];

  private searchSubject = new Subject<string>();
  private culture: string;

  constructor() {
    // navigator is not available while prerendering at build time.
    this.culture = (typeof navigator !== 'undefined' && navigator.language.split('-')[0]) || 'en';
  }

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(() => this.loadExercises());

    this.loadExercises();
  }

  onSearchInput(value: string): void {
    this.filterName = value;
    this.searchSubject.next(value);
  }

  toggleMuscle(option: MuscleOption): void {
    option.selected = !option.selected;
    this.loadExercises();
  }

  toggleTag(option: TagOption): void {
    option.selected = !option.selected;
    this.loadExercises();
  }

  toggleDifficulty(option: DifficultyOption): void {
    option.selected = !option.selected;
    this.loadExercises();
  }

  toggleFiltersExpanded(): void {
    this.filtersExpanded.update(v => !v);
  }

  clearFilters(): void {
    this.filterName = '';
    this.muscleGroups.forEach(g => g.options.forEach(o => o.selected = false));
    this.tagOptions.forEach(o => o.selected = false);
    this.difficultyOptions.forEach(o => o.selected = false);
    this.loadExercises();
  }

  getActiveFilterCount(): number {
    const muscleCount = this.muscleGroups.reduce((sum, g) => sum + g.options.filter(o => o.selected).length, 0);
    return muscleCount
      + this.tagOptions.filter(o => o.selected).length
      + this.difficultyOptions.filter(o => o.selected).length
      + (this.filterName ? 1 : 0);
  }

  trackByValue(index: number, option: { value: number }): number {
    return option.value;
  }

  trackByLabel(index: number, group: MuscleGroupSection): string {
    return group.label;
  }

  trackByExerciseId(index: number, exercise: ExercisePreview): number {
    return exercise.codeNum;
  }

  private loadExercises(): void {
    const selectedMuscles = this.muscleGroups.flatMap(g => g.options.filter(o => o.selected).map(o => o.value));
    const selectedTags = this.tagOptions.filter(o => o.selected).map(o => o.value);
    const selectedDifficulty = this.difficultyOptions.filter(o => o.selected).map(o => o.value);

    this.store.dispatch(ExerciseActions.loadExercises({
      selectedMuscles,
      selectedTags,
      selectedDifficulty,
      filterName: this.filterName,
      twoLetterCulture: this.culture
    }));
  }

  createNew(): void {
    this.router.navigate(['/exercise/new']);
  }
}
