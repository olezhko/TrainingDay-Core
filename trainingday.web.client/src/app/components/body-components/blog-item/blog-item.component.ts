import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BackendService } from '../../../services/backend/backend.service';
import { BlogDetails } from '../../../data/blog/blog-details.model';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-blog-item',
  templateUrl: './blog-item.component.html',
  styleUrls: ['./blog-item.component.css']
})
export class BlogItemComponent {
  blogPost!: BlogDetails;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private backendService: BackendService,
    public authService: AuthService
  ) { }

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.backendService.getBlogPost(id).subscribe(post => this.blogPost = post);
  }

  edit(): void {
    this.router.navigate(['/blogs/edit', this.blogPost.id]);
  }

  delete(): void {
    this.backendService.deleteBlogPost(this.blogPost.id).subscribe({
      complete: () => this.router.navigate(['/blogs'])
    });
  }
}
