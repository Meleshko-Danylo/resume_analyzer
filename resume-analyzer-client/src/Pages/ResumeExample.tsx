import React from 'react';
import Header from "../Components/Header";
import resume_example_1 from "../assets/resume-example-1.jpg"

const ResumeExample = () => {
    const handleDownloadPdf = () => {
        
    }
    
    return (
        <div className="App-wrapper">
            <Header/>
            <div className="App flex items-center justify-center p-4">
                <div className={"rounded-3xl w-[95%] p-2 flex gap-6 items-center justify-center flex-col"}>
                    <div className={"flex items-center gap-6"}>
                        <a className={"btn-2"} target={"_blank"} href={"https://docs.google.com/document/d/1ByUspXvRdAvTaocqyzqkBv6dP_MjT7T_B5JcCC6TJyI/edit?tab=t.0"}>Google docs</a>
                        <a target="_blank" href={"/Minimalist_Resume_Template.pdf"} className={"btn-2"} onClick={handleDownloadPdf}>Open pdf</a>
                        
                    </div>
                    <img src={resume_example_1} alt=""/>
                    {/*<div className={"w-[55%]"}>*/}
                    {/*    <embed*/}
                    {/*        type={"application/pdf"}*/}
                    {/*        src="/Minimalist_Resume_Template.pdf#toolbar=0&view=Fit"*/}
                    {/*        width="100%"*/}
                    {/*        height="700px"*/}
                    {/*        style={{ border: 'none' }}*/}
                    {/*    />*/}
                    {/*</div>*/}
                </div>
            </div>
        </div>
    );
};

export default ResumeExample;