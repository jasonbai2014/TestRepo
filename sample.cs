        public string Index()
        {
            string surl = "https://github.com/jasonbai2014/TestRepo";
            string lpath = "repo directory";
            CloneOptions co = new CloneOptions();
            Credentials ca = new UsernamePasswordCredentials() { Username = "username", Password = "PAT" };
            co.CredentialsProvider = (url, formUrl, types) => ca;
            Repository.Clone(surl, lpath, co);

            using (Repository repo = new Repository(lpath))
            {
                string localBranchName = "test-branch";
                string remoteBranchName = "origin/test-branch";
                Branch trackedBranch = repo.Branches[remoteBranchName];
                Branch localBranch = repo.CreateBranch(localBranchName, trackedBranch.Tip);
                repo.Branches.Update(localBranch, b => b.TrackedBranch = trackedBranch.CanonicalName);
                Commands.Checkout(repo, localBranch);

                RepositoryStatus status = repo.RetrieveStatus();
                if (status.IsDirty)
                {
                    StageChanges(repo);
                    CommitChanges(repo, "jason", "jason@yahoo.com");
                    PushChanges(repo);
                }
            }

            return "Hello";
        }

        public void StageChanges(Repository repo)
        {
            try
            {
                Commands.Stage(repo, "*");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:RepoActions:StageChanges " + ex.Message);
            }
        }

        public void CommitChanges(Repository repo, string username, string email)
        {
            try
            {
                repo.Commit("updating files..!!!", new Signature(username, email, DateTimeOffset.Now),
                    new Signature(username, email, DateTimeOffset.Now));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:RepoActions:CommitChanges " + e.Message);
            }
        }

        public void PushChanges(Repository repo)
        {
            try
            {
                var remote = repo.Network.Remotes["origin"];
                var options = new PushOptions();
                Credentials ca = new UsernamePasswordCredentials() { Username = "username", Password = "PAT" };

                options.CredentialsProvider = (url, formUrl, types) => ca;
                repo.Network.Push(remote, @"refs/heads/test-branch", options);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:RepoActions:PushChanges " + e.Message);
            }
        }
