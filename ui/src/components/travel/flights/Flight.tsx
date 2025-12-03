
import type { FlightOptionDto } from "../../../types/dto/flight.dto";

interface FlightProps {
    flight: FlightOptionDto;
}

const Flight = ({ flight }: FlightProps) => {
    return (
        <div>
            <h3>{flight.airline} {flight.flightNumber}</h3>
            <p>From: {flight.departure.airport} at {flight.departure.datetime}</p>
            <p>To: {flight.arrival.airport} at {flight.arrival.datetime}</p>
            <p>Duration: {flight.duration}</p>
            <p>Price: {flight.price.amount} {flight.price.currency}</p>
        </div>
    );
}

export default Flight;