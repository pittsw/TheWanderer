module Wanderer.ActiveGame

open System.Text.RegularExpressions

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Browser

open Wanderer.Modal
open Wanderer.Model
open Wanderer.Pages
open Wanderer.ViewHelpers

module R = Fable.Helpers.React
module P = Fable.Helpers.React.Props

let private modalLinkRegex = Regex(@"(\[\[\S+\]\])", RegexOptions.Compiled)

let private formatPageText (text : string) dispatch =
    let parts = modalLinkRegex.Split(text)
    printfn "%A" parts
    let resultParts =
        parts
        |> Array.map (fun part ->
            let m = modalLinkRegex.Match(part)
            if m.Success then
                let matchPart = m.Groups.[1].Value
                let linkParts = matchPart.Substring(2, matchPart.Length - 4).Split([|'|'|])
                let displayName = Array.head linkParts
                let linkName = linkParts.[linkParts.Length - 1]
                showModalLinkByName linkName displayName dispatch
            else
                R.str part)
        |> List.ofArray
    R.div [] resultParts

let view (gameState : ActiveGameState) dispatch =
    let character = gameState.Character
    let page = gameState.Page
    R.div [P.Style [P.Display "flex"]] [
        R.table [] [
            R.tbody [] [
                R.tr [] [
                    R.th [P.Style [P.TextAlign "right"]] [R.str "Might"]
                    R.td [] [R.str <| string character.Might]
                ]
                R.tr [] [
                    R.th [P.Style [P.TextAlign "right"]] [R.str "Will"]
                    R.td [] [R.str <| string character.Will]
                ]
                R.tr [] [R.td [P.ColSpan 2.; P.Style [P.Height "1em"]] []]
                R.tr [] [
                    R.th [P.Style [P.TextAlign "right"]] [R.str "Persuasion"]
                    R.td [] [R.str <| string character.Persuasion]
                ]
                R.tr [] [
                    R.th [P.Style [P.TextAlign "right"]] [R.str "Combat"]
                    R.td [] [R.str <| string character.Combat]
                ]
                R.tr [] [
                    R.th [P.Style [P.TextAlign "right"]] [R.str "Ritual"]
                    R.td [] [R.str <| string character.Ritual]
                ]
                R.tr [] [
                    R.th [P.Style [P.TextAlign "right"]] [R.str "Sneaking"]
                    R.td [] [R.str <| string character.Sneaking]
                ]
            ]
        ]
        R.div [P.ClassName "verticalDivider"] []
        R.div [P.ClassName "storyArea"] [
            R.p [] [formatPageText page.Text dispatch]
            R.ul [] [
                for cont in page.Continuations do
                    yield R.li [] [
                        cont.Description
                        R.br []
                        R.button [P.OnClick (fun _ -> dispatch (Flip cont.NextPageName))] [R.str "Choose"]]
            ]
        ]
    ]