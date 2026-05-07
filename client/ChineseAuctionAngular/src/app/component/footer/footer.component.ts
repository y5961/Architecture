import { Component } from '@angular/core';
import { environment } from '../../../../environment';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  imageUrl = environment.apiUrl + '/images/main/';
}

export { FooterComponent as Footer };
