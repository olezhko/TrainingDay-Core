import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-download-page',
  templateUrl: './download-page.component.html',
  styleUrls: ['./download-page.component.css']
})
export class DownloadPageComponent implements OnInit {

  ngOnInit(): void {
    const userAgent = navigator.userAgent || navigator.vendor || (window as any).opera;

    if (/android/i.test(userAgent)) {
      window.location.href = 'https://play.google.com/store/apps/details?id=com.OLASoft.TrainingDay&pli=1';
      return;
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !(window as any).MSStream) {
      window.location.href = 'https://apps.apple.com/us/app/workouts-trainingday/id1548560346';
      return;
    }

    // Otherwise show fallback
  }
}