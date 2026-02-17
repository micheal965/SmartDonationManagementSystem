import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-footer',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './user-footer.component.html',
  styleUrl: './user-footer.component.scss',
})
export class UserFooterComponent {}
