import { Component } from '@angular/core';
import { BackendService } from '../../../services/backend/backend.service';
import { BlogPreview } from '../../../data/blog/blog-preview.model';
import { PageEvent } from '@angular/material/paginator';
import { Router } from '@angular/router';

@Component({
  selector: 'app-blog-list',
  templateUrl: './blog-list.component.html',
  styleUrls: ['./blog-list.component.css']
})
export class BlogListComponent {
  blogPosts: BlogPreview[] = [];
  page = 1;
  pageSize = 5;
  culture = 'en';

  constructor(private backendService: BackendService, private router: Router) { }

  ngOnInit() {
    this.loadPosts();
    const clientCulture = navigator.language || navigator['language'];
    this.culture = clientCulture.split('-')[0];
  }

  loadPosts() {
    this.backendService.getBlogPosts(this.culture, this.page, this.pageSize)
      .subscribe(data => {
        console.log(data);
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
