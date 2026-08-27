import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BackendService } from '../../../services/backend/backend.service';
import { BlogDetails } from '../../../data/blog/blog-details.model';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-blog-item',
  standalone: true,
  imports: [CommonModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './blog-item.component.html',
  styleUrls: ['./blog-item.component.css']
})
export class BlogItemComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private backendService = inject(BackendService);
  private destroyRef = inject(DestroyRef);
  public authService = inject(AuthService);

  blogPost = signal<BlogDetails | null>(null);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.backendService.getBlogPost(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(post => this.blogPost.set(post));
  }

  edit(): void {
    this.router.navigate(['/blogs/edit', this.blogPost()?.id]);
  }

  delete(): void {
    const id = this.blogPost()?.id;
    if (id == null) return;
    this.backendService.deleteBlogPost(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        complete: () => this.router.navigate(['/blogs'])
      });
  }
}
