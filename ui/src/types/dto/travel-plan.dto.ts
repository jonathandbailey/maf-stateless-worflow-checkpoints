import type { FlightPlanDto } from "./flight-plan.dto";


export interface TravelPlanDto {
    destination: string;
    startDate: string;
    endDate: string;
    origin: string;
    flightPlan: FlightPlanDto;
}
