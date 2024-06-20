namespace GeneratorAPI.Services
{
    public static class OpenAICommands
    {
        public static string GenerateYoutubeTitleCommand(string topic, string contentType)
        {
            string command = $@"
                        As an expert YouTuber with 10 Years Experience.
                    your task is to generate 10 YouTube video titles based on the <input-topic> and content-type is <content-type> and ensure high CTR for my upcoming video.

                    First, use your expertise to develop the first 5 titles,
                    ensuring they are engaging, accurately represent the video content,
                    and abide by YouTube’s optimal practices for high CTR.

                    For the remaining five, pick 5 templates that best fit the video’s theme from the given list and use them to craft the titles.

                    Templates List:
                    -How To Not (Unwanted Outcome)-(Encouraging Words)!!
                    -The Simple (Task) that (Defeated) EVERYONE Except (Authority Figure)
                    -6 TOP (Objects) to Save You From (Unwanted Event)
                    -(Objects) I Never (Action) (Current Year)
                    -(Activity) Challenge That Will Change Your Life (30 DAYS RESULTS)
                    -12 (Objects) that can (Achieve Goal)
                    -[Achieve Goal] on [Platform] (easy [Activity] for beginners!)
                    -[Time Frame] Killer [Activity] ([Benefit])
                    -How to (Achieve Goal) in (Time Frame) [by (Current Year)]
                    -20 (Timely Object) You Should Never (Action)
                    -How A Pro (Position) ACTUALLY (Achieves Goal)
                    -End of (Entity)? (New Solution)! – Complete Guide
                    -Top 3 (Timely Action) NOW ([Bonus Reason To Take Action Now])
                    -(Activity/Goal) at (Obstacle) with (Simple Action)
                    -Why I choose (Option) over (Opposite Option)
                    -You’re doing (Activity) WRONG.
                    -I saw my boss do these 10 things in (Platform)!
                    -This NEW Approach to (Activity) Will Change Your (Possession) FOREVER!
                    -Never (unwanted action) Again – How To (accomplish goal) the EASY Way
                    -The (Negative Event) That Will Change A (Audience Segment)
                    -(Industry) Could Change Forever
                    -7 Things (Niche) Experts Do That You Probably Don’t
                    -Why does (Intriguing Title & Thumbnail Story)?
                    -How I (Achieved Goal) My First Calendar Year on (Platform)
                    -Why I Stopped (Popular Action) – What I Learned
                    -4 ONE-MINUTE Habits That (Achieve Goal) | (Activity) For (Specific Audience)
                    -Building My (Ultimate Goal) For/With (Constraint)
                    -(Problem)? 4 Common Mistakes To Avoid
                    -Asking Strangers Their Thoughts On (Subject)
                    -A (Tool) a day to get (Goal) to stay?? | (Solution) for (Problem)
                    -(Activity) When Something Wild Happened!
                    -100 Most Common [Objects] ([Support For Objects])
                    -6 Reasons (Pain Point)
                    -🤩 7 NEW (Place or Object) HACKS – PUT TO THE TEST! 🤩
                    -(Activity) The World’s Best (Object) in (Place)
                    -How I’d Start a (New Project) in (Upcoming Year)
                    -(Activity) without (Problem)
                    -Do (Emotional Question)?
                    -11 Hacks To Make Your (Possession) Look (Desired Adjective)
                    -(Activity) in (Extreme Environment)
                    -After watching video you will not (Common Activity)!
                    -5 (unique timeline) (activities) to Start In 2021 (extra proof)
                    -The (Positive Attribute) (Position)’s Road to (Huge Goal)
                    -Blowing The Lid on The Biggest (Industry) Secret
                    -WHY (Current Technology) WON’T LAST. (Technology) COMPARISON. What should you (Use)?
                    -(Desirable Entities) I Had No Idea Existed!
                    -(New Technology) Changes Everything – So much more than (Old Technology)!
                    -7 Ways To (Achieve Goal)
                    -The Secret Weapon That Got Me (Big Achievement)
                    -if I Were Starting A [Project] In 2022, This Is What I’d Do (7 Steps)
                    -Stop [Common Action]! (why you’re [Unwanted Result])
                    -the UGLY truth of (your profession or goal) you don’t see…
                    -Affordable (Tools or Objects) At (Unexpected Place) For Less Than (Dollar Amount)
                    -10 Odd (Activities) And What They Actually Mean
                    -How I plan to (Achieve Goal) and (Achieve Goal) in one year | level up
                    -2 [Activity] Every [Season] To [Achieve Goal] ([Keywords])
                    -You’re Probably (Activity) Wrong, and it Could be Dangerous (& [Other Unwanted Effect])
                    -5 Lazy (Goal, Activity, or Desire)!
                    (Achieve Goal) In (Short Time Frame) | (Subject)
                    -Switch off (Setting) now!
                    -14 Things [Subject] HATE (#1 Might Surprise You)
                    -I QUIT (Good Thing) After Learning 3 Things
                    -How to Buy Your First [Asset] (Step-By-Step)
                    -How to (reach goal) !! (additional benefit)! (refute an objection)
                    -Top 8 (Tools) For (Goal) In (Current Year)
                    -Asking (Who Your Audience Wants To Be Like) How to (Achieve Goal)
                    -(Problems) Are NO Match for this (Solution) ([Reach Goal])
                    -Skills You Won’t Learn In (Popular Training Ground)
                    -Top 20 (Tool) Tips for (Benefit)!
                    -5 MYTHS about (Activity) / (Activity) vs. Reality // Tips & Tricks // (Activity)
                    -I got 4 new (Niche) tools and gadgets! Let’s see what we think.
                    -THIS Simple Trick (Achieves Goal)
                    -The 7 Best (Entities) to (Activity) with (Trend)
                    -4 (Entity) Changes That Will Have a BIG IMPACT in (Next Year)
                    -Fix (Problem). Before It’s Irreversible

                    show the all results as a one list don't add any blank line betweem 5 and 6

                    <input-topic> = {topic}
                    <content-type> = {contentType}
                        ";

            return command;
        }

        public static string GenerateKeywordSearchToolCommand(string keyword)
        {
            string command = $@"
                   As an SEO expert specializing in keyword research, your objective is to create a well-rounded content plan for a specific target keyword. This task involves the creation of a comprehensive and strategic content plan drawn from your expertise in SEO and compliance with recent Google Quality guidelines and Google E-A-T rules.
                    Your content plan should encompass the following components:
                    List of 60 to 75-character related keyword and generate at least 16-20 responses involving the main keyword. Be sure to implement attention-grabbing, click-through-rate (CTR) driven keyword. Refrain from using quotation marks around the content, then according to each keyword show the search volume of related keyword (low, medium ,high ), then according to each related keyword show the rank difficulty (easy, moderate, hard, very hard )
                    For each item of the list the format is : Related Keyword (remove quotation marks) , search volume a decimal number (considering Low as 1-3, Medium as 4-6, High as 9-10) , rank difficulty
                    Bear in mind, the end reader will find the content beneficial, instantly valuable, and easy to read. Your plan should lure clicks and promptly answer the searcher’s intent. Retain your creativity and attention to detail while adhering to all specified guidelines and requirements.
                    Then show the average search volume of list as a decimal number (considering Low as 1-3, Medium as 4-6, High as 7-10) in another line
                    Then show the average search meter  of list (low, medium ,high ) in another line
                    Then show the average rank difficulty  of list (easy, moderate, hard, very hard ) in another line
                    Then create a list of analysis on how to make your primary keyword attention-grabbing, click-through-rate (CTR) driven keyword, display them as an ordered list. Enumerate 10. Make all responses to JSON format as I will be using the data to integrate with my API. Refer to this example. Make the structure of response exactly like this:
                    {{
                      ""average_search_volume"": """",
                      ""average_search_meter"": """",
                      ""average_rank_difficulty"": """",
                      ""attention_grabbing_tips"": [
                        """",
                        """"
                      ],
                      ""related_keywords"": [
                        {{
                          ""Keyword"": """",
                          ""search_volume"": """",
                          ""rank_difficulty"": """"
                        }},
                        {{
                          ""Keyword"": """",
                          ""search_volume"": """",
                          ""rank_difficulty"": """"
                        }},
                        {{
                          ""Keyword"": ""
                          ""search_volume"": """",
                          ""rank_difficulty"": """"
                        }}
                      ]
                    }}

                Target keyword: {keyword}
                                ";

            return command;
        }

        public static string GenerateHookGeneratorCommand(string idea, string contentType)
        {
            string command = $@"
                 If my idea is about {idea} and the content-type is {contentType}, Generate the following 1. Intriguing Question, 2. Visual Imagery 3. Quotation. Make it look like crafted with the precision of a seasoned digital marketer, this hook is designed to captivate attention across all types of content. Make it as  json response with property names: intriguing_question, visual_imagery, quotation."";
                            ";

            return command;
        }
    }
}