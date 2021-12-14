
 Write-Host "Listing branches..."
 git branch
 $shouldCommit = Read-Host -Prompt "Do you want to commit the application on the current  branch? (type  'y' or  'n')"
 while ($shouldCommit -eq $null){
    $shouldCommit = Read-Host -Prompt "Do you want to commit the application on the current  branch? (type  'y' or  'n')"
 
 }
 if  ($shouldCommit -eq 'y'){
   

        $commitMessage=    Read-Host -Prompt  'Please type commit  message'
         git add .
         git commit -m $commitMessage
    
 }


 $current_branch_string = Get-Content ".git/HEAD"
 $current_branch        =  $current_branch_string.Replace("ref: refs/heads/","")
 
 if ($current_branch -ne "master" ){
    $branch_response =  Read-Host -Prompt "Do you want to merge to the local master branch? (type  'y' or  'n')"
     while ($branch_response -eq $null){
      $branch_response =  Read-Host -Prompt "Do you want to merge to the local master branch? (type  'y' or  'n')"
     }

    if ($branch_response  -eq 'y'){
        Write-Host "Running:  git merge  $current_branch"
        git merge  $current_branch 
    } 
 }

 $push_response=  Read-Host -Prompt "Do you want to push to the remote master branch? (type  'y' or  'n')"
 while ($push_response -eq $null){
       $push_response=  Read-Host -Prompt "Do you want to push to the remote master branch? (type  'y' or  'n')"
 }
 if($push_response -eq 'y' ){
  
   $filepath ="$pwd\remote_repo.txt"
    Write-Host "Checking saved remote repo in $pwd\remote_repo.txt "
   if([System.IO.File]::Exists($filepath)){
        $remote_repo_url = Get-Content $filepath
      }


      Write-Host  "Saved remote repo "$remote_repo_url
      if($remote_repo_url -eq $null){
          $remote_repo_url  =  Read-Host -Prompt  "Please type the URL of the remote repository"

           while ($remote_repo_url -eq $null){
              $remote_repo_url  =  Read-Host -Prompt  "Please type the URL of the remote repository"

           }

           echo $remote_repo_url | Out-File -FilePath "$pwd\remote_repo.txt"
        }

        $remote_repo_name = git remote

        if($remote_repo_name -eq $null -Or $remote_repo_name -eq 'n'){
        
          Write-Host "git remote add origin $remote_repo_url"
          git remote add origin $remote_repo_url

        }
       
        $push_branch =  Read-Host -Prompt  "Please type the name of the branch you wish to push" 
         while ($push_branch -eq $null){
            $push_branch =  Read-Host -Prompt  "Please type the name of the branch you wish to push" 
         }
         Write-Host "Running command git push -u origin $push_branch..." 
         git push -u origin $push_branch
         Write-Host "checking remote branch: " 
         git remote 


    }

cd %cd%
%cd%\bin\db2s3.exe 