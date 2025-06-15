import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-faq',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './faq.component.html',
  styleUrl: './faq.component.scss'
})
export class FaqComponent {
  isOpenOne: boolean = false;
  isOpenTwo: boolean = false;
  isOpenThree: boolean = false;
  isOpenFour: boolean = false;
  isOpenFive: boolean = false;
}
