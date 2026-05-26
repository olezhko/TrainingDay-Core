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
      new ImageAbout('../../assets/EN/1.png', 'Preloaded Workouts', 'Start immediately with ready-made training programs for any fitness level.'),
      new ImageAbout('../../assets/EN/2.png', 'Big Exercise Collection', 'Browse 100+ exercises with technique guides, muscle targets, and tips.'),
      new ImageAbout('../../assets/EN/3.png', 'Exercise Brief Description', 'Get step-by-step instructions for correct form and execution.'),
      new ImageAbout('../../assets/EN/5.png', 'Exercise Filter', 'Find exercises by muscle group, tags, or difficulty level in seconds.'),
      new ImageAbout('../../assets/EN/6.png', 'Workout Performing Control', 'Track sets, reps, and rest time as you work through each session.'),
      new ImageAbout('../../assets/EN/7.png', 'Previous Workouts', 'Review your training history and monitor progress over time.'),
      new ImageAbout('../../assets/EN/8.png', 'Body Control', 'Log weight, waist, hips, and other measurements to see your transformation.'),
      new ImageAbout('../../assets/EN/4.png', 'Account Statistics', 'Visualize your overall activity and achievements in one place.'),
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
