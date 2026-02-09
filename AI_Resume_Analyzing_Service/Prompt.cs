namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public static class Prompt
{
    public static string Value = """
                                 I need you to analyze this resume, look whether it has all the necessary information,
                                 analyze whether the listed skills and certificates align with the provided experience and summary (analyze where the resume contains only relevant information), check if the summary is short enough, and if the information is written in the best way possible. 
                                 ,look at the way points are made in the resume. You should analyze whether the certificates are outdated or not (let's say if a certificate was made 5 years ago from the current day, 
                                 you should say that the person behind the resume has to make sure the certificate is not outdated). 
                                 Analyze whether the structure of the resume is clean, and give the overall score for the resume from 0 to 100.
                                 
                                 After you've looked at the resume and analyzed it, I need you to provide the result of your analysis in the following format (json). I'm leaving you comments with additional info to help you understand better what to write (they start from "//", I need you to remove them from your response):
                                 
                                 {
                                     overallScore: number, // your overall score for the resume after analyzing it
                                     shortSummary: string, // shot summary that explains what's good in the resume and what's bad
                                     suggestions: [ // each suggestion is a small object that has a score from 0 to 100, good and bad aspects of the category and small tips you can give to improve this part of the resume 
                                         { category: string score: number, good: string, bad: string, tip: string}, //category - summary
                                         education: {category: string, score: number, good: string, bad: string, tip: string}, //category - education
                                         experience: {category: string, score: number, good: string, bad: string, tip: string}, //category - experience
                                         skills: {category: string, score: number, good: string, bad: string, tip: string} //category - skills
                                         ...
                                     ]
                                 }  
                                 """;
}