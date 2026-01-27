import React, {useRef, useState} from 'react';
import {analyze_resume} from "../api/analyze_resume";
import {Request} from "../Core/Request";
import {useAnalyzerContext} from "./AnalyzerContextProvider";

const ResumeContainer = () => {
    const resumeFileInputRef: any = useRef<HTMLInputElement>(null);
    const {setLoading, setAnalysisResults} = useAnalyzerContext();
    const [inputKey, setInputKey] = useState(0);

    const handleClick = ():void => {
        if(resumeFileInputRef.current) {
            resumeFileInputRef.current.click();
        }
    }

    const handleChange = async (event:React.ChangeEvent<HTMLInputElement>):Promise<void> => {
        try {
            if(!event.target.files || event.target.files.length === 0) return;
            setLoading(true);
            
            const request:Request = {
                resume: event.target.files[0],
                position_description: ""
            };

            setInputKey(prevState => prevState + 1);

            const response = await analyze_resume(request);
            setAnalysisResults(response);
        }
        catch (e) {
            console.error(e);
        }
        finally {
            setLoading(false);
        }
    }

    return (
        <div className={"resume-container"}>
            <input ref={resumeFileInputRef} key={inputKey} type="file" className="resume-file-input"
                   accept={"application/pdf"} onChange={handleChange}/>
            <button className={"resume-file-btn"} onClick={handleClick}>Add Resume</button>
        </div>
    );
};

export default ResumeContainer;