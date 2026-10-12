import { ChangeDetectionStrategy, Component, OnInit, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-download-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './download-page.component.html',
  styleUrls: ['./download-page.component.css']
})
export class DownloadPageComponent implements OnInit {
  private platformId = inject(PLATFORM_ID);

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const userAgent = navigator.userAgent || navigator.vendor || (window as any).opera;

    // Let search engine crawlers (Googlebot smartphone reports Android) index this page.
    if (/bot|crawler|spider|crawling|lighthouse/i.test(userAgent)) {
      return;
    }

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