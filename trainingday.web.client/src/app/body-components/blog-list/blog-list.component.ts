import { Component } from '@angular/core';
import { BackendService } from '../../services/backend/backend.service';
import { BlogPreview } from '../../data/blog/blog-preview.model';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-blog-list',
  templateUrl: './blog-list.component.html',
  styleUrls: ['./blog-list.component.css']
})
export class BlogListComponent {
  blogPosts: BlogPreview[] = [];
  page = 1;
  pageSize = 5;
  culture = 'en'; // You can make this dynamic

  constructor(private backendService: BackendService) { }

  ngOnInit() {
    this.loadPosts();
  }

  loadPosts() {
    this.backendService.getBlogPosts(this.culture, this.page, this.pageSize)
      .subscribe(data => this.blogPosts = data);
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

  onPageChange(event: PageEvent) {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadPosts();
  }
}
