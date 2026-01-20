import {Request} from "../Core/Request";
import {axiosInstance} from "../index";

export const analyze_resume = async (request: Request) => {
    try {
        const response = await axiosInstance.post("resume-analyzer-api/analyze-resume", request);
        return response.data;
    }
    catch (error) {
        console.log(error);
    }
}