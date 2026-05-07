import { Component } from '@angular/core';
import { RouterOutlet,RouterLink } from "@angular/router";

@Component({
  selector: 'app-manager',
  imports: [RouterOutlet,RouterLink],
  templateUrl: './manager.component.html',
  styleUrl: './manager.component.scss'
})
export class ManagerComponent {

}
