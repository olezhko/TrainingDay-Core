import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SelectItem } from 'src/app/data/selectItem.interface';
import { BackendService } from 'src/app/services/backend/backend.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-exercise-edit',
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.css']
})
export class ExerciseEditComponent implements OnInit {
  @Input() id: number = 0;

  muscles!: SelectItem[];
  tags!: SelectItem[];

  selectedMuscles!: SelectItem[];
  selectedTags!: SelectItem[];

  editExerciseForm!: FormGroup;

  constructor(private backendService: BackendService, private router: Router, private fb: FormBuilder) { }

  ngOnInit(): void {
    this.editExerciseForm = this.fb.group({
      name: [null, Validators.required],
      code: [null, Validators.required],
      selectedMuscles: [null, Validators.required],
      selectedTags: [null, Validators.required],
      startDescription: [null, Validators.required],
      executionDescription: [null, Validators.required],
      adviceDescription: [null, Validators.required],
    });

    this.backendService.getExerciseEditParams('en').subscribe(params => {
      this.muscles = params.allMuscles;
      this.tags = params.allTags;
      this.editExerciseForm.controls['code'].setValue(params.offeredCode);
    });
  }

  save(): void {
    if (this.id === 0) {
      this.backendService.createExercise(this.editExerciseForm.value).subscribe(data => {
        this.router.navigate(['/exercises']);
      });

    } else {
      this.backendService.editExercise(this.editExerciseForm.value.id, this.editExerciseForm.value).subscribe(data => {
        this.router.navigate(['/exercises']);
      });
    }
  }
}
