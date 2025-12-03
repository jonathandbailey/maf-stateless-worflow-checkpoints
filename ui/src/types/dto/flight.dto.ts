export interface FlightSearchResultDto {
    artifactKey: string;
    results: FlightOptionDto[];
}

export interface FlightOptionDto {
    airline: string;
    flightNumber: string;
    departure: FlightEndpointDto;
    arrival: FlightEndpointDto;
    duration: string;
    price: PriceDto;
}

export interface FlightEndpointDto {
    airport: string;
    datetime: string;
}

export interface PriceDto {
    amount: number;
    currency: string;
}
