import React from 'react';
import Header from "../Components/Header";
import ResumeExample from "../Components/ResumeExample";

const ResumeExamples = () => {
    return (
        <div className="App-wrapper">
            <Header/>
            <div className="App flex flex-col items-center justify-center p-2">
                <ResumeExample googleDocsUrl={"https://docs.google.com/document/d/1ByUspXvRdAvTaocqyzqkBv6dP_MjT7T_B5JcCC6TJyI/edit?tab=t.0"}
                    pdfFileUrl={"/Minimalist_Resume_Template.pdf"}
                />
                <ResumeExample googleDocsUrl={"https://docs.google.com/document/d/1EujuYFWxVXZ2PUaJ2uizvK5raMoMsz1KMys-UYpUSk4/edit?tab=t.0"}
                               pdfFileUrl={"/MCS_Resume_Template_(Bullet Points).pdf"}
                />
                <ResumeExample pdfFileUrl={"/Blue_Neutral_Simple_Minimalist_Professional_Web_Developer_Resume.pdf"}/>
            </div>
        </div>
    );
};

export default ResumeExamples;