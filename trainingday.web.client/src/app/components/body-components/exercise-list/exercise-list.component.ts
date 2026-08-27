import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { ExerciseTags, ExerciseTagsEnglishLabels } from 'src/app/data/exercises/exercise-tags';
import { MUSCLE_GROUPS, MusclesEnumEnglishLabels, MusclesEnum } from 'src/app/data/exercises/exercise-muscle';
import { BackendService } from 'src/app/services/backend/backend.service';
import { AuthService } from 'src/app/services/auth/auth.service';
import { ExercisePreviewComponent } from '../exercise-preview/exercise-preview.component';

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
  private backendService = inject(BackendService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  public authService = inject(AuthService);

  public filterName: string = '';
  public exercisePreviews = signal<ExercisePreview[]>([]);
  public loading = signal(false);
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
    this.culture = navigator.language.split('-')[0] || 'en';
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
    const muscles = this.muscleGroups.flatMap(g => g.options.filter(o => o.selected).map(o => o.value));
    const tags = this.tagOptions.filter(o => o.selected).map(o => o.value);
    const difficulties = this.difficultyOptions.filter(o => o.selected).map(o => o.value);

    this.loading.set(true);
    this.backendService.getExercises(muscles, tags, difficulties, this.filterName, this.culture)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: exercises => {
          this.exercisePreviews.set(exercises);
          this.loading.set(false);
        },
        error: () => { this.loading.set(false); }
      });
  }

  createNew(): void {
    this.router.navigate(['/exercise/new']);
  }
}
