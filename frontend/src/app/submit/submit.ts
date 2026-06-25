import { Component, inject, Inject } from '@angular/core';
import { HttpClient, HttpParams, HttpParameterCodec } from '@angular/common/http';
import { Router } from '@angular/router';
import { SubmitInfo } from '../models/submit.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Button } from '../button/button';
import { Block } from '../block/block';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-submit',
  imports: [Button, Block, ReactiveFormsModule],
  templateUrl: './submit.html',
  styleUrl: './submit.css',
})
export class Submit {
  private http = inject(HttpClient);
  private router = inject(Router); // Inject the Router service

  name = new FormControl('');
  link = new FormControl('');
  additionalInfo = new FormControl('');
  contacts = new FormControl('');

  submitAccepted: boolean = false;

  constructor(@Inject(DOCUMENT) private document: Document) {}
  ngOnInit() {
    this.document.body.classList.add('min-h-screen');
    this.document.body.classList.add('bg-[url(/background.png)]');
    this.document.body.classList.add('bg-repeat');
  }

  ngOnDestroy() {
    this.document.body.classList.remove('min-h-screen');
    this.document.body.classList.remove('bg-[url(/background.png)]');
    this.document.body.classList.remove('bg-repeat');
  }

  submit() {
    if (
      this.name.value != null &&
      this.link.value != null &&
      this.additionalInfo.value != null &&
      this.contacts.value != null
    ) {
      const formData = new FormData();

      formData.append('name', this.name.value);
      formData.append('contact', this.contacts.value);
      formData.append('link', this.link.value);
      formData.append('additionalInfo', this.additionalInfo.value);

      this.http.post('https://oknoweb.ru/submit/api/new', formData).subscribe((result) => {
        this.submitAccepted = true;
        const id = JSON.parse(result.toString());
        this.router.navigate([`/submit/success/${id}`]);
      });
    }
  }
}
