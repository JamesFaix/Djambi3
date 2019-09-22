﻿namespace Djambi.Api.Logic.Managers

open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic.Interfaces

type SearchManager(searchRepo : ISearchRepository) =
    interface ISearchManager with        
        member x.searchGames query session =
            searchRepo.searchGames query