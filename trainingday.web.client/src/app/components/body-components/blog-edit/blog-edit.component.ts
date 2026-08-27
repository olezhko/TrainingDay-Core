import { ChangeDetectionStrategy, Component, DestroyRef, OnDestroy, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Editor, NgxEditorModule, Toolbar, Validators as EditorValidators } from 'ngx-editor';
import { BackendService } from '../../../services/backend/backend.service';
import { Router, ActivatedRoute  } from '@angular/router';
import { BlogPostEditViewModel } from 'src/app/data/blog/blog-preview.model';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-blog-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgxEditorModule, MatSnackBarModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './blog-edit.component.html',
  styleUrls: ['./blog-edit.component.css']
})
export class BlogEditComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private sanitizer = inject(DomSanitizer);
  private backendService = inject(BackendService);
  public router = inject(Router);
  private route = inject(ActivatedRoute);
  private snackBar = inject(MatSnackBar);
  private destroyRef = inject(DestroyRef);

  id: number = 0;
  isEditMode = false;

  blogForm!: FormGroup;
  toolbar: Toolbar = [
    ['bold', 'italic'],
    ['underline', 'strike'],
    ['code', 'blockquote'],
    ['ordered_list', 'bullet_list'],
    [{ heading: ['h1', 'h2', 'h3', 'h4', 'h5', 'h6'] }],
    ['link', 'image'],
    ['text_color', 'background_color'],
    ['align_left', 'align_center', 'align_right', 'align_justify'],
  ];

  availableTags: string[] = [];
  filteredTags: string[] = [];
  blogPost!: BlogPostEditViewModel;
  editorPreview!: Editor;
  editorContent!: Editor;

  ngOnInit(): void {
    this.editorPreview = new Editor();
    this.editorContent = new Editor();
    this.initializeForm();
    const id = this.route.snapshot.paramMap.get('id');
    this.id = id ? Number(id) : 0;
    this.loadBlogDetails(this.id);
  }

  initializeForm(): void {
    this.blogForm = this.fb.group({
      id: [null],
      cultureId: [null],
      blogId: [null],
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [EditorValidators.required()]],
      author: ['', [Validators.required]],
      date: [new Date(), [Validators.required]],
      view: ['', [EditorValidators.required()]],
      tags: [[], [Validators.required]]
    });
  }

  loadBlogDetails(id: number): void {
    this.isEditMode = !!this.id && this.id !== 0;

    if (this.isEditMode) {
      this.backendService.getBlogPost(id)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: data => {
            this.blogPost = data;

            this.blogForm.patchValue({
              id: this.blogPost.id,
              cultureId: this.blogPost.cultureId,
              blogId: this.blogPost.blogId,
              title: this.blogPost.title,
              description: this.blogPost.description,
              author: this.blogPost.author,
              date: this.blogPost.date,
              view: this.blogPost.view,
              tags: this.blogPost.tags
            });
          },
          error: (err) => console.error('Failed to fetch blog post', err)
        });
    }
  }

  trackByTag(index: number, tag: string): string {
    return tag;
  }

  onTagInput(event: any): void {
    const query = event.target.value.toLowerCase();
    if (!query) {
      this.filteredTags = [];
      return;
    }
    this.filteredTags = this.availableTags.filter(tag =>
      tag.toLowerCase().includes(query) &&
      !this.blogForm.value.tags.includes(tag)
    );
  }

  onTagKeyDown(event: KeyboardEvent) {
    const inputEl = event.target as HTMLInputElement;
    const input = inputEl.value.trim();

    if (event.key === 'Enter' && input) {
      event.preventDefault();
      this.addTag(input); // This will add even if it's a new custom tag
      inputEl.value = ''; // Clear input
      this.filteredTags = [];
    }
  }

  addTag(tag: string): void {
    const currentTags = this.blogForm.value.tags;
    if (!currentTags.includes(tag)) {
      this.blogForm.patchValue({
        tags: [...currentTags, tag]
      });
    }
    this.filteredTags = [];
  }

  removeTag(tag: string): void {
    this.blogForm.patchValue({
      tags: this.blogForm.value.tags.filter((t: string) => t !== tag)
    });
  }

  onSubmit(): void {
    if (!this.blogForm.valid) {
      this.blogForm.markAllAsTouched();
      return;
    }

    const v = this.blogForm.value;
    const blogModel = Object.assign(new BlogPostEditViewModel(), {
      id: v.id, title: v.title, description: v.description,
      tags: v.tags, author: v.author, date: v.date,
      view: v.view, blogId: v.blogId, cultureId: v.cultureId,
    });

    const request$ = this.isEditMode
      ? this.backendService.editBlogPost(this.id, blogModel)
      : this.backendService.createBlogPost(blogModel);

    request$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
      next: () => {
        this.snackBar.open('Blog post saved successfully!', 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/blogs']);
      },
      error: () => {
        this.snackBar.open('Failed to save blog post.', 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
        });
      }
    });
  }

  getSafeHtml(html: string) {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  ngOnDestroy(): void {
    this.editorContent.destroy();
    this.editorPreview.destroy();
  }

  onDelete(): void {
    if (this.isEditMode) {
      this.backendService.deleteBlogPost(this.id)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          complete: () => this.router.navigate(['/blogs'])
        });
    }
  }
}
