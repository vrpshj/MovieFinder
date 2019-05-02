import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, config } from 'rxjs';
import { movie } from 'src/app/models/movie';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { retry } from 'rxjs/operators';
import { PriceData } from 'src/app/models/PriceData';
import { movieDetails } from 'src/app/models/movieDetails';
export interface PriceDialogData {
  title: string;
  cheapestPrice: string;
  provider: string;
  id: string;
}

@Component({
  selector: 'movies',
  templateUrl: './movies.component.html',
  
  styleUrls: ['./movies.component.css']
})
export class Movies implements OnInit{
  title = 'Webjet Movie Finder';
  subtitle = 'Portal to find movies for cheapest price';


  datetime = Date.now();
  response: any;
  movielist: movie[];
  error: any;
  pricedata: any;
  moviedetails: any;
  search = '';
  loading = false;
  constructor(private http: HttpClient, public dialog: MatDialog ) { }


  ngOnInit() {
    try {
      var d = this.getAllMovies();
    } catch (error) { this.loading = false; console.log('Unable to get movies, please retry');}
  }

  getAllMovies() {
    this.loading = true;

    return this.http.get<string>('/api/Movies/GetAvailableMovies')
      .pipe(
        retry(3)
    ).subscribe((data: any) => {
      this.movielist = data.data
      this.loading = false;
    }, error => { this.error = error; this.loading = false; });
  }
  //Clearing the search and reloading all available movies
  ClearSearch()
  {
    this.search = '';
    try {
      var d = this.getAllMovies();
    } catch (error) { this.loading = false; console.log('Unable to get movies, please retry'); }
  }
  //Filtering the movies with specified text
  filterMovies() {
    this.loading = true;
    try {
    var req = this.http.get<string>('/api/Movies/GetAvailableMovies?SearchText=' + this.search)
      .pipe(
        retry(3)// retry a failed request up to 3 times
      )
      .subscribe((data: any) => {
        this.movielist = data.data;
        this.loading = false;
      }, error => { this.error = error; this.loading = false; });
    } catch (error) { this.loading = false; console.log('Unable to get movies, please retry'); }
  }
  //Get movie cheapest price
  getMovieCheapestPrice(name: string)
  {
    this.loading = true;
    try{
    var d = this.http.get<string>('api/movies/GetMovieCheapestPriceByName?Name=' + name).subscribe((data: any) => {
      this.pricedata = data.data
      this.loading = false;
      this.openPriceDialog();
    }, error => { this.error = error; this.loading = false; });
  } catch(error) { this.loading = false; console.log('Unable to get movies, please retry'); }
  }
  //Open Movie price dialog to display the price and provider
  openPriceDialog(): void {
    const dialogRef = this.dialog.open(PriceDialog, {
      width: '400px',
      hasBackdrop: true,
      data: this.pricedata
    });

  }
  //Get movie details
  getMovieDetails(id: string) {
    this.loading = true;
    try {
    var d = this.http.get<string>('api/movies/GetMovieDetailsByID?ID=' + id).subscribe((data: any) => {
      this.moviedetails = data.data;
      this.loading = false;
      this.openDetailsDialog();
    }, error => { this.error = error; this.loading = false; });
  } catch(error) { this.loading = false; console.log('Unable to get movies, please retry'); }
  }
  //Open movie details dialog to display details
  openDetailsDialog(): void {
    const dialogRef = this.dialog.open(DetailsDialog, {
      width: '600px',
      hasBackdrop: true,
      data: this.moviedetails
    });

  }

}

@Component({
  selector: 'PriceDialog',
  templateUrl: 'price-dialog.html',
})
export class PriceDialog {

  constructor(
    public dialogRef: MatDialogRef<PriceDialog>,
    @Inject(MAT_DIALOG_DATA) public data: movieDetails) { }

  onCloseClick(): void {
    this.dialogRef.close();
  }

}


@Component({
  selector: 'DetailsDialog',
  templateUrl: 'details-dialog.html',
})
export class DetailsDialog {

  constructor(
    public dialogRef: MatDialogRef<DetailsDialog>,
    @Inject(MAT_DIALOG_DATA) public data: PriceData) { }

  onCloseClick(): void {
    this.dialogRef.close();
  }

}

