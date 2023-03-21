import { Component } from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  constructor(private http: HttpClient) {
  }

  public downloadPDF(): any {
    var mediaType = 'application/pdf';
    this.http.get('https://localhost:7000/linkedin/certificate', { responseType: 'blob' }).subscribe(
      (res) => {
        let blob = new Blob([res], { type: 'application/pdf' });
        let pdfUrl = window.URL.createObjectURL(blob);

        var PDF_link = document.createElement('a');
        PDF_link.href = pdfUrl;
        //   TO OPEN PDF ON BROWSER IN NEW TAB
        window.open(pdfUrl, '_blank')
      },
      e => { console.log(e); }
    );
  }

  postOnLinkedIn() {
    this.http.post<linkedInAuthResponse>('https://localhost:7000/linkedin', {}).subscribe(
      (res) => {location.href = res.url},
      (error) => console.log(error));
  }

  postTextOnLinkedIn() {
    this.http.post<linkedInAuthResponse>('https://localhost:7000/linkedin', {}).subscribe(
      (res) => {location.href = res.url},
      (error) => console.log(error));
  }
}

interface linkedInAuthResponse{
  url: string
}
