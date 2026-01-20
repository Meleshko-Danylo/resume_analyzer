import React, { useRef } from 'react';
import {analyze_resume} from "../api/analyze_resume";
import {Request} from "../Core/Request";
import {Response} from "../Core/Response";

interface ResumeContainerProps {
    onAnalysisComplete: (results: Response) => void;
}

const ResumeContainer = ({ onAnalysisComplete }: ResumeContainerProps) => {
    const resumeFileInputRef: any = useRef<HTMLInputElement>(null);

    const handleClick = ():void => {
        if(resumeFileInputRef.current) {
            resumeFileInputRef.current.click();
        }
    }

    const handleChange = async (event:React.ChangeEvent<HTMLInputElement>):Promise<void> => {
        if(!event.target.files || event.target.files.length === 0) return;
        const request:Request = {
            resume: event.target.files[0],
        };
        const response = await analyze_resume(request);
        onAnalysisComplete(response);
    }

    return (
        <div className={"resume-container"}>
            <input ref={resumeFileInputRef} type="file" className="resume-file-input"
                   accept={"application/pdf"} onChange={handleChange}/>
            <button className={"resume-file-btn"} onClick={handleClick}>Add Resume</button>
        </div>
    );
};

export default ResumeContainer;