import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { ExerciseTags, ExerciseTagsEnglishLabels } from 'src/app/data/exercises/exercise-tags';
import { MUSCLE_GROUPS, MusclesEnumEnglishLabels, MusclesEnum } from 'src/app/data/exercises/exercise-muscle';
import { BackendService } from 'src/app/services/backend/backend.service';

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

@Component({
  selector: 'app-exercise-list',
  templateUrl: './exercise-list.component.html',
  styleUrls: ['./exercise-list.component.css']
})
export class ExerciseListComponent implements OnInit, OnDestroy {
  public filterName: string = '';
  public exercisePreviews: ExercisePreview[] = [];
  public loading = false;
  public filtersExpanded = false;

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

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();
  private culture: string;

  constructor(private backendService: BackendService, private router: Router) {
    this.culture = navigator.language.split('-')[0] || 'en';
  }

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => this.loadExercises());

    this.loadExercises();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
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

  clearFilters(): void {
    this.filterName = '';
    this.muscleGroups.forEach(g => g.options.forEach(o => o.selected = false));
    this.tagOptions.forEach(o => o.selected = false);
    this.loadExercises();
  }

  get activeFilterCount(): number {
    const muscleCount = this.muscleGroups.reduce((sum, g) => sum + g.options.filter(o => o.selected).length, 0);
    return muscleCount
      + this.tagOptions.filter(o => o.selected).length
      + (this.filterName ? 1 : 0);
  }

  private loadExercises(): void {
    const muscles = this.muscleGroups.flatMap(g => g.options.filter(o => o.selected).map(o => o.value));
    const tags = this.tagOptions.filter(o => o.selected).map(o => o.value);

    this.loading = true;
    this.backendService.getExercises(muscles, tags, this.filterName, this.culture)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: exercises => {
          this.exercisePreviews = exercises;
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
  }

  createNew(): void {
    this.router.navigate(['/exercise/new']);
  }
}
