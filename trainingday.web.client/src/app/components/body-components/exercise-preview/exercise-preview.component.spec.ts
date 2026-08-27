import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { ExercisePreviewComponent } from './exercise-preview.component';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';

describe('ExercisePreviewComponent', () => {
  let component: ExercisePreviewComponent;
  let fixture: ComponentFixture<ExercisePreviewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ExercisePreviewComponent],
      providers: [provideRouter([])]
    });
    fixture = TestBed.createComponent(ExercisePreviewComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('exercise', new ExercisePreview(1, 'Push Up', 100));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
