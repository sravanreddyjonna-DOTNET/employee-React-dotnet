pipeline {
    agent any

    environment {
        DOCKER_IMAGE      = 'sravanreddy98/employee-api'
        DOCKER_TAG        = "${env.BUILD_NUMBER}"
        DOCKER_LATEST_TAG = 'latest'
    }

    stages {

        // ─── Stage 1: Checkout ──────────────────────────────────────────
        stage('Checkout') {
            steps {
                checkout scm
                echo "Branch: ${env.GIT_BRANCH} | Build #: ${env.BUILD_NUMBER}"
            }
        }

        // ─── Stage 2: Restore packages ─────────────────────────────────
        stage('Restore') {
            steps {
                bat 'dotnet restore EmployeeApi.sln'
            }
        }

        // ─── Stage 3: Build ─────────────────────────────────────────────
        stage('Build') {
            steps {
                bat 'dotnet build EmployeeApi.sln --configuration Release --no-restore'
            }
        }

        // ─── Stage 4: Test (skips gracefully if no test projects exist) ─
        stage('Test') {
            steps {
                script {
                    def found = bat(
                        script: '@dir /s /b *Tests.csproj *Test.csproj 2>nul',
                        returnStatus: true
                    )
                    if (found == 0) {
                        bat 'dotnet test EmployeeApi.sln --configuration Release --no-build --logger "trx;LogFileName=test-results.trx"'
                        junit '**/test-results.trx'
                    } else {
                        echo 'No test projects found — skipping test stage.'
                    }
                }
            }
        }

        // ─── Stage 5: Docker Build ──────────────────────────────────────
        stage('Docker Build') {
            steps {
                bat """
                    docker build ^
                        -t %DOCKER_IMAGE%:%DOCKER_TAG% ^
                        -t %DOCKER_IMAGE%:%DOCKER_LATEST_TAG% ^
                        .
                """
            }
        }

        // ─── Stage 6: Docker Push ───────────────────────────────────────
        // FIX 1: Removed the "when" block — it was silently skipping this
        //        stage every time because env.GIT_BRANCH is unreliable in
        //        a standard Pipeline job. Add it back once push is working.
        stage('Docker Push') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-credentials',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    powershell """
                        \$ErrorActionPreference = 'Stop'

                        # Bypass Docker Desktop credential helper
                        # (docker-credential-desktop conflicts with Jenkins
                        # running as LocalSystem on Windows)
                        \$env:DOCKER_CONFIG = "\$env:WORKSPACE\\.docker-tmp"
                        New-Item -ItemType Directory -Force -Path \$env:DOCKER_CONFIG | Out-Null

                        try {
                            # FIX 2: Use -p flag directly instead of pipe.
                            # PowerShell pipes send .NET objects not raw bytes,
                            # which breaks --password-stdin silently.
                            docker login -u \$env:DOCKER_USER -p \$env:DOCKER_PASS

                            # FIX 3: Check exit code — don't push if login failed
                            if (\$LASTEXITCODE -ne 0) {
                                throw "Docker login failed (exit code \$LASTEXITCODE)"
                            }
                            Write-Host "Login succeeded"

                            docker push ${DOCKER_IMAGE}:${DOCKER_TAG}
                            if (\$LASTEXITCODE -ne 0) {
                                throw "docker push tag failed (exit code \$LASTEXITCODE)"
                            }

                            docker push ${DOCKER_IMAGE}:${DOCKER_LATEST_TAG}
                            if (\$LASTEXITCODE -ne 0) {
                                throw "docker push latest failed (exit code \$LASTEXITCODE)"
                            }

                            Write-Host "Successfully pushed ${DOCKER_IMAGE}:${DOCKER_TAG}"
                        }
                        finally {
                            docker logout
                            Remove-Item -Recurse -Force \$env:DOCKER_CONFIG -ErrorAction SilentlyContinue
                        }
                    """
                }
            }
        }
    }

    post {
        always {
            // Clean up local images after push to save disk space
            bat """
                docker rmi %DOCKER_IMAGE%:%DOCKER_TAG% 2>nul || exit /b 0
                docker rmi %DOCKER_IMAGE%:%DOCKER_LATEST_TAG% 2>nul || exit /b 0
            """
            cleanWs()
        }
        success {
            echo "Build ${env.BUILD_NUMBER} PASSED — pushed ${DOCKER_IMAGE}:${DOCKER_TAG}"
        }
        failure {
            echo "Build ${env.BUILD_NUMBER} FAILED — check the console output above"
        }
    }
}