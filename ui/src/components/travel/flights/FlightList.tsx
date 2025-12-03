import type { FlightOptionDto } from "../../../types/dto/flight.dto";
import Flight from "./flight";

interface FlightListProps {
    flights: FlightOptionDto[];
}

const FlightList = ({ flights }: FlightListProps) => {
    return (
        <div>
            <h2>Available Flights</h2>
            {flights.length === 0 ? (
                <p>No flights available</p>
            ) : (
                <div>
                    {flights.map((flight, index) => (
                        <Flight key={index} flight={flight} />
                    ))}
                </div>
            )}
        </div>
    );
}

export default FlightList;