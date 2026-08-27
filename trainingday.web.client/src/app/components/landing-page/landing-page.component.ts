import { ChangeDetectionStrategy, Component } from '@angular/core';
import { JoinUsComponent } from './join-us/join-us.component';
import { AboutUsComponent } from './about-us/about-us.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [JoinUsComponent, AboutUsComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent {
}
