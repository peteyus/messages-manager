import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(private router: Router) { }

  ngOnInit() {
    //setTimeout(() => this.router.navigate(['conversations']), 5000);
  }
}
