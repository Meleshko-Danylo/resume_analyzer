import {Request} from "../Core/Request";
import {axiosInstance} from "../index";
import {CancelToken} from "axios";

export const analyze_resume = async (request: Request, ct: CancelToken) => {
    try {
        const data = new FormData();
        data.append("resume", request.resume);
        data.append("positionDescription", request.position_description ?? "")
        
        const response = await axiosInstance.post("resume-analyzer-api/analyze-resume", data, {
            cancelToken: ct
        });
        return response.data;
    }
    catch (error) {
        console.log(error);
    }
}