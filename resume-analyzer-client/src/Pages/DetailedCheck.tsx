import React from 'react';
import Header from "../Components/Header";
import ResumeContainer, {AnalyzerType} from "../Components/ResumeContainer";
import AnalyzerResults from "../Components/AnalyzerResults";

const DetailedCheck = () => {

    return (
        <div className="App-wrapper ">
            <Header/>
            <div className="App flex-col xl:flex-row 2xl:h-[95vh]">
                <ResumeContainer analyzer_type={AnalyzerType.Detailed}/>
                <AnalyzerResults />
            </div>
        </div>
    );
};

export default DetailedCheck;