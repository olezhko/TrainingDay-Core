import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlogItemComponent } from './blog-item.component';

describe('BlogItemComponent', () => {
  let component: BlogItemComponent;
  let fixture: ComponentFixture<BlogItemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BlogItemComponent]
    });
    fixture = TestBed.createComponent(BlogItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
