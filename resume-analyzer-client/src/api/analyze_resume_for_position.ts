import {Request} from "../Core/Request";
import {axiosInstance} from "../index";

export const analyze_resume_for_position = async (request:Request) => {
    try {
        const result = await axiosInstance.post("resume-analyzer-api/analyze-resume-for-position", request);
    }
    catch (error) {
        console.log(error);
    }
}