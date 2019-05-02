import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
 // title = 'Movie Finder';
  //subtitle = 'Portal to find movies for cheapest price';
  title = 'Compliance Checkpoint';
  subtitle = 'Finding Audits';
  datetime = Date.now();
}
