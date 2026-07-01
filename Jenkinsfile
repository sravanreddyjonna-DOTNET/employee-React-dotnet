pipeline {
    agent any

    environment {
        DOCKER_IMAGE      = 'sravanreddy98/employee-api'
        DOCKER_TAG        = "${env.BUILD_NUMBER}"
        DOCKER_LATEST_TAG = 'latest'
        SONAR_HOST_URL    = 'http://localhost:9000'
        SONAR_PROJECT_KEY = 'employee-api'
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

        // ─── Stage 3: SonarQube Analysis + Build ───────────────────────
        // SonarScanner wraps the build: begin → build → end
        stage('SonarQube Analysis') {
            steps {
                withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
                    bat """
                        dotnet-sonarscanner begin ^
                            /k:"%SONAR_PROJECT_KEY%" ^
                            /d:sonar.host.url="%SONAR_HOST_URL%" ^
                            /d:sonar.token="%SONAR_TOKEN%" ^
                            /d:sonar.scm.disabled=true

                        dotnet build EmployeeApi.sln --configuration Release --no-restore

                        dotnet-sonarscanner end ^
                            /d:sonar.token="%SONAR_TOKEN%"
                    """
                }
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
        stage('Docker Push') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-credentials',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    powershell """
                        \$ErrorActionPreference = 'Stop'

                        \$env:DOCKER_CONFIG = "\$env:WORKSPACE\\.docker-tmp"
                        New-Item -ItemType Directory -Force -Path \$env:DOCKER_CONFIG | Out-Null

                        try {
                            docker login -u \$env:DOCKER_USER -p \$env:DOCKER_PASS
                            if (\$LASTEXITCODE -ne 0) { throw "Docker login failed" }

                            docker push ${DOCKER_IMAGE}:${DOCKER_TAG}
                            if (\$LASTEXITCODE -ne 0) { throw "docker push tag failed" }

                            docker push ${DOCKER_IMAGE}:${DOCKER_LATEST_TAG}
                            if (\$LASTEXITCODE -ne 0) { throw "docker push latest failed" }

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
