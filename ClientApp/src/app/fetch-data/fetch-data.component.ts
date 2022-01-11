import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public lodz: WeatherForecast[];
  public warsaw: WeatherForecast[];
  public szczecin: WeatherForecast[];
  public wroclaw: WeatherForecast[];
  public gdansk: WeatherForecast[];
  public krakow: WeatherForecast[];
  public suwalki: WeatherForecast[];
  public rzeszow: WeatherForecast[];
  public poland: WeatherForecast[];
  public days: string;
  http: HttpClient;
  baseUrl: string;
  apiKey: string;
  headers: any;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.headers = new Headers();
    this.apiKey = this.generateKey();
    const headers = { 'ApiKey': this.apiKey };
    
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Lodz&d=3', { headers }).subscribe(result => {
      this.lodz = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Warsaw&d=1', { headers }).subscribe(result => {
      this.warsaw = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Szczecin&d=1', { headers }).subscribe(result => {
      this.szczecin = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Wroclaw&d=1', { headers }).subscribe(result => {
      this.wroclaw = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Gdansk&d=1', { headers }).subscribe(result => {
      this.gdansk = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Krakow&d=1', { headers }).subscribe(result => {
      this.krakow = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Suwalki&d=1', { headers }).subscribe(result => {
      this.suwalki = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/average?c=Rzeszow&d=1', { headers }).subscribe(result => {
      this.rzeszow = result;
    }, error => console.error(error));
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast/poland?d=1', { headers }).subscribe(result => {
      this.poland = result;
    }, error => console.error(error));
    http.get<string>(baseUrl + 'weatherforecast/dbsize', { headers }).subscribe(result => {
      this.days = result;
    }, error => console.error(error));
  }

  generateKey(): string {
    let key = "";
    var date = new Date();
    date.setHours(0, 0, 0, 0);
    var ticks = ((date.getTime() * 10000) + 621355968000000000) - (date.getTimezoneOffset() * 600000000); //convert to C# time
    var test = ticks.toString();

    for (let i = 0; i < test.length - 1; i++) {
      var value = parseInt(test[i] + test[i + 1]);
      if (value != 0) {
        if (value >= 97) {
          value = value % 25;
          value = 97 + value;

        }
        else if (value >= 65) {
          value = value % 25;
          value = 65 + value;
        }
        else {
          value = value % 10;
          value = 48 + value;
        }
        key += String.fromCharCode(value);
      }
    }
    return key;
  }

  getData() {
    this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast/lodz').subscribe(result => {
      this.lodz = result;
    }, error => console.error(error));
  }
}

interface WeatherForecast {
  date: string;
  temp: number;
  pressure: number;
  humidity: number;
  precipitation: number;
  wind_speed: number;
  wind_direction: number;
}
