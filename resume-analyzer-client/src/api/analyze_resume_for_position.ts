import {Request} from "../Core/Request";
import {axiosInstance} from "../index";
import {CancelToken} from "axios";

export const analyze_resume_for_position = async (request:Request, ct: CancelToken) => {
    try {
        const data = new FormData();
        data.append("resume", request.resume);
        data.append("positionDescription", request.position_description ?? "")
        
        const result = await axiosInstance.post("resume-analyzer-api/analyze-resume-for-position", data, {
            cancelToken: ct
        });
        return result.data;
    }
    catch (error) {
        console.log(error);
    }
}