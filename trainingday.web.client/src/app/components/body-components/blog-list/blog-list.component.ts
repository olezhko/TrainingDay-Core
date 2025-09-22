import { Component, Input } from '@angular/core';
import { BackendService } from '../../../services/backend/backend.service';
import { BlogPreview } from '../../../data/blog/blog-preview.model';
import { PageEvent } from '@angular/material/paginator';
import { Router,ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-blog-list',
  templateUrl: './blog-list.component.html',
  styleUrls: ['./blog-list.component.css']
})
export class BlogListComponent {
  @Input() culture: string = 'en';
  blogPosts: BlogPreview[] = [];
  page = 1;
  pageSize = 5;

  constructor(private backendService: BackendService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.setCulture();
    this.loadPosts();
  }

  setCulture()  {
    const clientCulture = navigator.language || navigator['language'];

    var cultureFromRoute = this.route.snapshot.queryParams['cu'];
    this.culture = cultureFromRoute? cultureFromRoute : clientCulture.split('-')[0];
  }

  loadPosts() {
    this.backendService.getBlogPosts(this.culture, this.page, this.pageSize)
      .subscribe(data => {
        this.blogPosts = data}
      );
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
