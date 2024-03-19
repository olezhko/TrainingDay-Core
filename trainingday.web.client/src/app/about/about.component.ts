import { Component, OnInit } from '@angular/core';
import { ImageAbout } from '../data/about/image-about.model';
import { NgbCarouselModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-about',
  standalone: true,
	imports: [NgbCarouselModule,CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
  images: ImageAbout[] = [];
  
  ngOnInit(): void {
    this.images = [
      new ImageAbout('../../assets/EN/1.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/2.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/3.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/4.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/5.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/6.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/7.png', 'Контактная информация'),
      new ImageAbout('../../assets/EN/8.png', 'Контактная информация'),
    ]
  };
}
