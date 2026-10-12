import { DOCUMENT } from '@angular/common';
import { Injectable, inject } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { ActivatedRouteSnapshot, NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

export interface SeoData {
  title?: string;
  description?: string;
  image?: string;
  type?: 'website' | 'article';
  noindex?: boolean;
}

export const SITE_URL = 'https://trainingday.space';
const SITE_NAME = 'TrainingDay';
const DEFAULT_DESCRIPTION = 'TrainingDay is a free workout tracker app. Plan workouts, track progress and explore an exercise library with technique guides and target muscles.';
const DEFAULT_IMAGE = `${SITE_URL}/assets/main.png`;
const DESCRIPTION_MAX_LENGTH = 160;

@Injectable({ providedIn: 'root' })
export class SeoService {
  private title = inject(Title);
  private meta = inject(Meta);
  private router = inject(Router);
  private document = inject(DOCUMENT);

  /** Applies static SEO data declared in route `data.seo` on every navigation. */
  init(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => this.update(this.getRouteSeo(this.router.routerState.snapshot.root)));
  }

  /** Call from components that load content asynchronously (blog post, exercise). */
  update(data: SeoData): void {
    const title = data.title ? `${data.title} | ${SITE_NAME}` : `${SITE_NAME} – Workout Tracker & Exercise Library`;
    const description = SeoService.toDescription(data.description) || DEFAULT_DESCRIPTION;
    const url = SITE_URL + this.router.url.split(/[?#]/)[0];
    const image = data.image || DEFAULT_IMAGE;

    this.title.setTitle(title);
    this.meta.updateTag({ name: 'description', content: description });
    this.meta.updateTag({ name: 'robots', content: data.noindex ? 'noindex, nofollow' : 'index, follow' });

    this.meta.updateTag({ property: 'og:title', content: title });
    this.meta.updateTag({ property: 'og:description', content: description });
    this.meta.updateTag({ property: 'og:url', content: url });
    this.meta.updateTag({ property: 'og:image', content: image });
    this.meta.updateTag({ property: 'og:type', content: data.type ?? 'website' });
    this.meta.updateTag({ name: 'twitter:title', content: title });
    this.meta.updateTag({ name: 'twitter:description', content: description });
    this.meta.updateTag({ name: 'twitter:image', content: image });

    this.setCanonical(url);
  }

  /** Strips HTML and trims text to a meta-description friendly length. */
  static toDescription(text?: string | null): string {
    if (!text) return '';
    const plain = text.replace(/<[^>]*>/g, ' ').replace(/&nbsp;/g, ' ').replace(/\s+/g, ' ').trim();
    if (plain.length <= DESCRIPTION_MAX_LENGTH) return plain;
    return plain.substring(0, plain.lastIndexOf(' ', DESCRIPTION_MAX_LENGTH - 1)) + '…';
  }

  private setCanonical(url: string): void {
    let link = this.document.head.querySelector<HTMLLinkElement>('link[rel="canonical"]');
    if (!link) {
      link = this.document.createElement('link');
      link.setAttribute('rel', 'canonical');
      this.document.head.appendChild(link);
    }
    link.setAttribute('href', url);
  }

  private getRouteSeo(route: ActivatedRouteSnapshot): SeoData {
    while (route.firstChild) {
      route = route.firstChild;
    }
    return route.data['seo'] ?? {};
  }
}
