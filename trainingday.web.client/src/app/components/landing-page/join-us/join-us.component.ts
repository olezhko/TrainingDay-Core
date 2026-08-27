import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-join-us',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './join-us.component.html',
  styleUrls: ['./join-us.component.css']
})
export class JoinUsComponent {
  heroBg = '../../assets/home-2.jpg';
}
