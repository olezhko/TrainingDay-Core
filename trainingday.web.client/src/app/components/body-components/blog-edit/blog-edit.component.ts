import { Component, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Editor, Toolbar, Validators as EditorValidators } from 'ngx-editor';   
import { BackendService } from '../../../services/backend/backend.service';
import { Router, ActivatedRoute  } from '@angular/router';
import { BlogPostEditViewModel } from 'src/app/data/blog/blog-preview.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-blog-edit',
  templateUrl: './blog-edit.component.html',
  styleUrls: ['./blog-edit.component.css']
})
export class BlogEditComponent {
  @Input() id: number = 0;

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
  constructor(
    private fb: FormBuilder,
    private sanitizer: DomSanitizer,
    private backendService: BackendService,
    private router: Router,
    private route: ActivatedRoute, 
    private snackBar: MatSnackBar
  ) {
  }

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
    if (id != 0) {
      this.backendService.getBlogPost(id)
        .subscribe(data => { 
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
        });
    }
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
    if (this.blogForm.valid) {
      if(this.blogForm.value.id != 0)
        this.backendService.editBlogPost(this.id, this.blogForm.value).subscribe(data => 
        {
          console.log(data);
        });
      else
        this.backendService.createBlogPost(this.blogForm.value).subscribe(data => 
        {
          console.log(data);
        });

      this.snackBar.open('Blog post saved successfully!', 'Close', {
        duration: 3000,
        horizontalPosition: 'right',
        verticalPosition: 'top',
        panelClass: ['success-snackbar']
      });

      this.router.navigate(['/blogs'])
    } else {
      this.blogForm.markAllAsTouched();
    }
  }

  getSafeHtml(html: string) {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  ngOnDestroy(): void {
    this.editorContent.destroy();
    this.editorPreview.destroy();
  }

}
