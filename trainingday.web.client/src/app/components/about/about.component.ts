import { Component, OnInit } from '@angular/core';
import { ImageAbout } from '../../data/about/image-about.model';
import { NgbCarouselModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [NgbCarouselModule, CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
  images: ImageAbout[] = [];
  currentIndex = 0;

  ngOnInit(): void {
    this.images = [
      new ImageAbout('../../assets/EN/1.png', 'Preloaded Workouts'),
      new ImageAbout('../../assets/EN/2.png', 'Big Exercise Collection'),
      new ImageAbout('../../assets/EN/3.png', 'Exercise Brief Description'),
      new ImageAbout('../../assets/EN/5.png', 'Exercise Filter'),
      new ImageAbout('../../assets/EN/6.png', 'Workout Perfoming Control'),
      new ImageAbout('../../assets/EN/7.png', 'Previous Workouts'),
      new ImageAbout('../../assets/EN/8.png', 'Body Control'),
      new ImageAbout('../../assets/EN/4.png', 'Account Statistic'),
    ]
  };

  next() {
    this.currentIndex = (this.currentIndex + 1) % this.images.length;
  }

  prev() {
    this.currentIndex = (this.currentIndex - 1 + this.images.length) % this.images.length;
  }

  goTo(index: number) {
    this.currentIndex = index;
  }
}
