import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-join-us',
  templateUrl: './join-us.component.html',
  styleUrls: ['./join-us.component.css']
})
export class JoinUsComponent {
  constructor(private router: Router) { }

  join() {
    this.router.navigate(['/download']);
  }
}
