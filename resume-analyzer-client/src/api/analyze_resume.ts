import {Request} from "../Core/Request";
import {axiosInstance} from "../index";

export const analyze_resume = async (request: Request) => {
    try {
        const data = new FormData();
        data.append("resume", request.resume);
        data.append("positionDescription", request.position_description ?? "")
        
        const response = await axiosInstance.post("resume-analyzer-api/analyze-resume", data);
        return response.data;
    }
    catch (error) {
        console.log(error);
    }
}