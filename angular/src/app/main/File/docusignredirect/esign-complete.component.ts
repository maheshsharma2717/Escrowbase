import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-esign-complete',
  templateUrl: './esign-complete.component.html',
  styleUrls: ['./esign-complete.component.css']
})
export class EsignCompleteComponent implements OnInit {
  envelopeId: string | null = null;

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.envelopeId = params['envelopeId'] || null;
    });

   
  }
}
