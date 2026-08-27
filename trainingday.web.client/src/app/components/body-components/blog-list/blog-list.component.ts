import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { BackendService } from '../../../services/backend/backend.service';
import { BlogPreview } from '../../../data/blog/blog-preview.model';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-blog-list',
  standalone: true,
  imports: [CommonModule, MatPaginatorModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './blog-list.component.html',
  styleUrls: ['./blog-list.component.css']
})
export class BlogListComponent implements OnInit {
  private backendService = inject(BackendService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  public authService = inject(AuthService);

  culture: string = 'en';
  blogPosts = signal<BlogPreview[]>([]);
  page = 1;
  pageSize = 5;

  ngOnInit() {
    this.setCulture();
    this.loadPosts();
  }

  setCulture() {
    const clientCulture = navigator.language || navigator['language'];

    const cultureFromRoute = this.route.snapshot.queryParams['cu'];
    this.culture = cultureFromRoute ? cultureFromRoute : clientCulture.split('-')[0];
  }

  loadPosts() {
    this.backendService.getBlogPosts(this.culture, this.page, this.pageSize)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(data => this.blogPosts.set(data));
  }

  trackByPostId(index: number, post: BlogPreview): number {
    return post.id;
  }

  nextPage() {
    this.page++;
    this.loadPosts();
  }

  previousPage() {
    if (this.page > 1) {
      this.page--;
      this.loadPosts();
    }
  }

  createNew(): void {
    this.router.navigate(['/blogs/new']);
  }

  onPageChange(event: PageEvent) {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadPosts();
  }
}
