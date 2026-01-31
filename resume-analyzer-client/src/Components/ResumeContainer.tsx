import React, {useRef, useState} from 'react';
import {analyze_resume} from "../api/analyze_resume";
import {Request} from "../Core/Request";
import {useAnalyzerContext} from "./AnalyzerContextProvider";
import {ToastContainer, toast, Bounce} from 'react-toastify';
import {analyze_resume_for_position} from "../api/analyze_resume_for_position";

export enum AnalyzerType {
    Standard = "standard",
    Detailed = "detailed",
}

type Details = {
    position: string;
    requirements: string;
}

const ResumeContainer = ({analyzer_type}:{analyzer_type: AnalyzerType}) => {
    const resumeFileInputRef = useRef<HTMLInputElement>(null);
    const positionRef = useRef<HTMLInputElement>(null);
    const requirementsRef = useRef<HTMLDivElement>(null);
    const {setLoading, setAnalysisResults, setLastStartedAt} = useAnalyzerContext();
    const [inputKey, setInputKey] = useState(0);
    
    const handleClick = ():void => {
        if(resumeFileInputRef.current) {
            resumeFileInputRef.current.click();
        }
    }

    const getRequirements = () => {
        if(!requirementsRef.current) return "";
        const editor = requirementsRef.current;
        let requirements: string = "Requirements:\n\n";
        
        editor.childNodes.forEach((node:any) => {
            if (node.nodeName === 'DIV') {
                node.childNodes.forEach((node:any) => {
                    requirements += node.wholeText ?? "";
                })
            }
            else {
                requirements += node.wholeText ?? "";
            }
        })
        
        return requirements;
    }
    
    const getPosition = () => {
        if(!positionRef.current) return "";
        const editor = positionRef.current;
        return `Position: ${editor.value}\n\n`;
    }
    
    const getResume = () => {
        if(!resumeFileInputRef.current || !resumeFileInputRef.current.files 
            || resumeFileInputRef.current.files.length <= 0) return;
        
        return resumeFileInputRef.current.files[0];
    }
    
    const handleSubmit = async () => {
        const startTime = Date.now();
        try {
            if(!positionRef.current || !requirementsRef.current || !resumeFileInputRef.current) return;
            
            const position = getPosition();
            const requirements = getRequirements();
            if(requirements.length > 1000) {toast.warn("Requirements contain too many characters"); return;}
            
            const resume = getResume();
            if(!resume) {toast.warn("Upload your resume"); return;}
            
            setLastStartedAt(startTime);
            setLoading(true);
            
            const request:Request = {
                resume: resume,
                position_description: `${position}${requirements}`
            };
            setInputKey(prevState => prevState + 1);
            
            const response = await analyze_resume_for_position(request);
            setAnalysisResults(response);
        }
        catch (e) {
            console.error(e);
            setLoading(false);
        }
        finally {
            setLoading(false);
        }
    }
    
    const handleChange = async (event:React.ChangeEvent<HTMLInputElement>):Promise<void> => {
        const startTime = Date.now();
        try {
            if(!event.target.files || event.target.files.length <= 0) return;
            setLastStartedAt(startTime);
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
            setLoading(false);
        }
        finally {
            setLoading(false);
        }
    }
    
    const handleOnPaste = (event: any) => {
        event.preventDefault();
        if(!event.clipboardData) return;
        const text = event.clipboardData.getData('text/plain');
        document.execCommand('insertText', false, text);
    }
    
    if(analyzer_type === AnalyzerType.Detailed){
        return (
            <div className={"resume-container h-full text-[#dbd8e3] gap-6 items-center justify-start"}>
                <div className={"w-full flex justify-end items-center pr-4"}>
                    <button onClick={handleSubmit} style={{boxShadow: "rgba(50, 50, 93, 0.25) 0px 30px 60px -12px inset, rgba(0, 0, 0, 0.3) 0px 18px 36px -18px inset"}} className={"bg-[#352f44] rounded-lg pt-2 pb-2 pl-5 pr-5 pointer hover:bg-[#5c5470]"}>
                        Submit
                    </button>
                </div>
                <input ref={resumeFileInputRef} key={inputKey} type="file" className="resume-file-input"
                       accept={"application/pdf"} />
                <button className={"resume-file-btn"} onClick={handleClick}>Add Resume</button>
                <div className={"flex flex-col w-4/5 gap-1"}>
                    <label className={""} htmlFor="application-position">Position</label>
                    <input ref={positionRef} type="text" className={"input"} id={"application-position"}/>
                </div>
                <div className={"flex flex-col w-4/5 gap-1"}>
                    <label htmlFor="editor">Requirements</label>
                    <div ref={requirementsRef} id={"editor"} className={"editor input"} contentEditable={true} onPaste={handleOnPaste}>
                        
                    </div>
                </div>
                <ToastContainer
                    position="top-right"
                    autoClose={3000}
                    hideProgressBar={false}
                    newestOnTop={false}
                    closeOnClick
                    rtl={false}
                    pauseOnFocusLoss
                    draggable
                    pauseOnHover={false}
                    theme="dark"
                    transition={Bounce}
                />
            </div>
        );
    }
    
    return (
        <div className={"resume-container h-full items-center justify-center"}>
            <input ref={resumeFileInputRef} key={inputKey} type="file" className="resume-file-input"
                   accept={"application/pdf"} onChange={handleChange}/>
            <button className={"resume-file-btn"} onClick={handleClick}>Add Resume</button>
        </div>
    );
};

export default ResumeContainer;