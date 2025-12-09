import type { TravelPlanDto } from "../../../types/dto/travel-plan.dto";

interface TravelPlanProps {
    travelPlan: TravelPlanDto | null;
}

const TravelPlan = ({ travelPlan }: TravelPlanProps) => {
    return (
        <div>{travelPlan?.origin} {travelPlan?.destination}</div>
    );
}
export default TravelPlan;